import { z } from 'zod';

export const SCHEMA_VERSION = 1;

export const UserIntroRecordSchema = z.object({
  userId:     z.string().min(1),
  userLogin:  z.string().min(1),
  soundFile:  z.string().min(1).optional(),
  gifFile:    z.string().min(1).optional(),
  enabled:    z.boolean(),
  notes:      z.string().optional(),
  updatedUtc: z.number().int().positive(),
});

export type UserIntroRecord = z.infer<typeof UserIntroRecordSchema>;
