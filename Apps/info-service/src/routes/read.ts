import { FastifyInstance } from 'fastify';
import { Collection } from '../store/collection';
import { z } from 'zod';

type Registry = Map<string, Collection<z.ZodTypeAny>>;

export async function readRoutes(app: FastifyInstance, opts: { registry: Registry }) {
  const { registry } = opts;

  app.get('/info/:collection', async (req, reply) => {
    const { collection } = req.params as { collection: string };
    const col = registry.get(collection);
    if (!col) return reply.status(404).send({ error: 'unknown collection' });
    return { records: col.getAll() };
  });

  app.get('/info/:collection/:key', async (req, reply) => {
    const { collection, key } = req.params as { collection: string; key: string };
    const col = registry.get(collection);
    if (!col) return reply.status(404).send({ error: 'unknown collection' });
    const record = col.get(key);
    if (record === undefined) return reply.status(404).send({ error: 'not found' });
    return record;
  });
}
