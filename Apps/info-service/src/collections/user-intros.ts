import * as path from 'node:path';
import { Collection } from '../store/collection';
import { UserIntroRecordSchema, SCHEMA_VERSION } from '../store/schemas/user-intros';

export const userIntros = new Collection({
  filePath: path.resolve(__dirname, '../../data/user-intros.json'),
  collectionName: 'user-intros',
  schemaVersion: SCHEMA_VERSION,
  recordSchema: UserIntroRecordSchema,
});
