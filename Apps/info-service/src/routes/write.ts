import { FastifyInstance } from 'fastify';
import { Collection } from '../store/collection';
import { z } from 'zod';

type Registry = Map<string, Collection<z.ZodTypeAny>>;

export async function writeRoutes(app: FastifyInstance, opts: { registry: Registry }) {
  const { registry } = opts;

  app.post('/info/:collection/:key', async (req, reply) => {
    const { collection, key } = req.params as { collection: string; key: string };
    const col = registry.get(collection);
    if (!col) return reply.status(404).send({ error: 'unknown collection' });
    try {
      await col.set(key, req.body);
    } catch (err: unknown) {
      return reply.status(400).send({ error: err instanceof Error ? err.message : String(err) });
    }
    return reply.status(201).send({ ok: true });
  });

  app.put('/info/:collection/:key', async (req, reply) => {
    const { collection, key } = req.params as { collection: string; key: string };
    const col = registry.get(collection);
    if (!col) return reply.status(404).send({ error: 'unknown collection' });
    try {
      await col.set(key, req.body);
    } catch (err: unknown) {
      return reply.status(400).send({ error: err instanceof Error ? err.message : String(err) });
    }
    return { ok: true };
  });

  app.delete('/info/:collection/:key', async (req, reply) => {
    const { collection, key } = req.params as { collection: string; key: string };
    const col = registry.get(collection);
    if (!col) return reply.status(404).send({ error: 'unknown collection' });
    const existing = col.get(key);
    if (existing === undefined) return reply.status(404).send({ error: 'not found' });
    await col.delete(key);
    return { ok: true };
  });
}
