import { z } from 'zod';

export const SCHEMA_VERSION = 1;

export const PendingIntroRecordSchema = z.object({
  userId:      z.string().min(1),
  userLogin:   z.string().min(1),
  redeemId:    z.string().min(1),
  redeemUtc:   z.number().int().positive(),
  rewardTitle: z.string().min(1),
  userInput:   z.string().optional(),
  status:      z.enum(['pending', 'fulfilled', 'rejected']),
  resolvedUtc: z.number().int().positive().optional(),
});

export type PendingIntroRecord = z.infer<typeof PendingIntroRecordSchema>;
