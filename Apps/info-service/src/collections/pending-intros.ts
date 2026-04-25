import * as path from 'node:path';
import { Collection } from '../store/collection';
import { PendingIntroRecordSchema, SCHEMA_VERSION } from '../store/schemas/pending-intros';

export const pendingIntros = new Collection({
  filePath: path.resolve(__dirname, '../../data/pending-intros.json'),
  collectionName: 'pending-intros',
  schemaVersion: SCHEMA_VERSION,
  recordSchema: PendingIntroRecordSchema,
});
