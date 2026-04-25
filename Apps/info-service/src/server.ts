import Fastify from 'fastify';
import {
  serializerCompiler,
  validatorCompiler,
} from 'fastify-type-provider-zod';
import { z } from 'zod';
import { Collection } from './store/collection';
import { readRoutes } from './routes/read';
import { writeRoutes } from './routes/write';

export function buildServer(opts: { collections?: Collection<z.ZodTypeAny>[] } = {}) {
  const app = Fastify({ logger: true });

  app.setValidatorCompiler(validatorCompiler);
  app.setSerializerCompiler(serializerCompiler);

  const registry = new Map<string, Collection<z.ZodTypeAny>>(
    (opts.collections ?? []).map((c) => [c.name(), c])
  );

  app.get('/health', async (_req, _reply) => {
    return { ok: true, uptime: process.uptime(), collections: [...registry.keys()] };
  });

  app.register(readRoutes, { registry });
  app.register(writeRoutes, { registry });

  return app;
}
