import { buildServer } from './server';
import { userIntros } from './collections/user-intros';
import { pendingIntros } from './collections/pending-intros';

const PORT = 8766;
const HOST = '127.0.0.1';

async function main() {
  await userIntros.load();
  await pendingIntros.load();

  const app = buildServer({ collections: [userIntros, pendingIntros] });

  await app.listen({ port: PORT, host: HOST });
  console.log(`info-service listening on ${HOST}:${PORT}`);
}

main().catch((err) => {
  console.error(err);
  process.exit(1);
});
