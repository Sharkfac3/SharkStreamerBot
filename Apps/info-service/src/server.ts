import Fastify from 'fastify';
import {
  serializerCompiler,
  validatorCompiler,
} from 'fastify-type-provider-zod';
import { z } from 'zod';
import { Collection } from './store/collection';
import { readRoutes } from './routes/read';
import { writeRoutes } from './routes/write';

const ALLOWED_CORS_ORIGINS = new Set([
  'http://127.0.0.1:5174',
  'http://127.0.0.1:4174',
  'http://localhost:5174',
  'http://localhost:4174',
]);

export function buildServer(opts: { collections?: Collection<z.ZodTypeAny>[] } = {}) {
  const app = Fastify({ logger: true });

  app.setValidatorCompiler(validatorCompiler);
  app.setSerializerCompiler(serializerCompiler);

  app.addHook('onRequest', async (req, reply) => {
    const origin = req.headers.origin;

    if (origin && ALLOWED_CORS_ORIGINS.has(origin)) {
      reply.header('Access-Control-Allow-Origin', origin);
      reply.header('Vary', 'Origin');
      reply.header('Access-Control-Allow-Methods', 'GET,POST,PUT,DELETE,OPTIONS');
      reply.header('Access-Control-Allow-Headers', 'Content-Type');
    }

    if (req.method === 'OPTIONS') {
      return reply.code(204).send();
    }
  });

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
