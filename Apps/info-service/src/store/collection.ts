import { z } from 'zod';
import * as fs from 'node:fs/promises';
import * as path from 'node:path';

export const EnvelopeSchema = <T extends z.ZodTypeAny>(recordSchema: T) =>
  z.object({
    schemaVersion: z.number().int().positive(),
    collection:    z.string().min(1),
    updatedUtc:    z.number().int().positive(),
    records:       z.record(z.string(), recordSchema),
  });

interface CollectionOptions<T extends z.ZodTypeAny> {
  filePath: string;
  collectionName: string;
  schemaVersion: number;
  recordSchema: T;
}

export class Collection<T extends z.ZodTypeAny> {
  private records: Record<string, z.infer<T>> = {};
  private filePath: string;
  private collectionName: string;
  private schemaVersion: number;
  private recordSchema: T;

  constructor(opts: CollectionOptions<T>) {
    this.filePath = opts.filePath;
    this.collectionName = opts.collectionName;
    this.schemaVersion = opts.schemaVersion;
    this.recordSchema = opts.recordSchema;
  }

  async load(): Promise<void> {
    try {
      await fs.access(this.filePath);
    } catch {
      return;
    }

    const raw = JSON.parse(await fs.readFile(this.filePath, 'utf-8'));
    const result = EnvelopeSchema(this.recordSchema).safeParse(raw);

    if (!result.success) {
      console.error(`[${this.collectionName}] load failed — zod errors:`, result.error.issues);
      process.exit(1);
    }

    if (result.data.schemaVersion !== this.schemaVersion) {
      console.error(
        `[${this.collectionName}] schema version mismatch — on disk: ${result.data.schemaVersion}, expected: ${this.schemaVersion}`
      );
      process.exit(1);
    }

    if (result.data.collection !== this.collectionName) {
      console.error(
        `[${this.collectionName}] collection name mismatch — on disk: "${result.data.collection}", expected: "${this.collectionName}"`
      );
      process.exit(1);
    }

    this.records = result.data.records;
  }

  getAll(): Record<string, z.infer<T>> {
    return { ...this.records };
  }

  get(key: string): z.infer<T> | undefined {
    return this.records[key];
  }

  async set(key: string, record: z.infer<T>): Promise<void> {
    const result = this.recordSchema.safeParse(record);
    if (!result.success) {
      throw new Error(result.error.message);
    }
    this.records[key] = result.data;
    await this.writeAtomic();
  }

  async delete(key: string): Promise<void> {
    if (!(key in this.records)) return;
    delete this.records[key];
    await this.writeAtomic();
  }

  name(): string {
    return this.collectionName;
  }

  private async writeAtomic(): Promise<void> {
    const envelope = {
      schemaVersion: this.schemaVersion,
      collection: this.collectionName,
      updatedUtc: Date.now(),
      records: this.records,
    };
    await fs.mkdir(path.dirname(this.filePath), { recursive: true });
    const tmpPath = this.filePath + '.tmp';
    await fs.writeFile(tmpPath, JSON.stringify(envelope, null, 2), 'utf-8');
    await fs.rename(tmpPath, this.filePath);
  }
}

export type CollectionRecord<T extends z.ZodTypeAny> = z.infer<T>;
