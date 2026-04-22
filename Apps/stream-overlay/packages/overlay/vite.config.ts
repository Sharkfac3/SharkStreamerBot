import { defineConfig } from 'vite';

export default defineConfig(({ command }) => ({
  // base: './' is required for OBS local file loading (production build only).
  // When OBS loads dist/index.html as a local file, all asset paths must be relative.
  // Dev server uses '/' so Vite serves public/ static files correctly instead of index.html.
  base: command === 'build' ? './' : '/',

  build: {
    outDir: 'dist',
    assetsDir: 'assets',

    // Produce a clean dist/ on each build — prevents stale OBS assets.
    emptyOutDir: true,
  },

  server: {
    // Dev server URL: http://localhost:5173
    // During development, point the OBS browser source here instead of a local file.
    port: 5173,
    strictPort: true, // Fail fast if 5173 is occupied — no silent port fallback.
  },
}));
