import { useEffect, useState } from 'react';

interface HealthResponse {
  ok: boolean;
  uptime: number;
  collections: string[];
}

export default function HealthPage() {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | undefined>();
  const [data, setData] = useState<HealthResponse | undefined>();

  useEffect(() => {
    fetch('http://127.0.0.1:8766/health')
      .then((res) => {
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        return res.json() as Promise<HealthResponse>;
      })
      .then((json) => setData(json))
      .catch((err: unknown) =>
        setError(err instanceof Error ? err.message : String(err))
      )
      .finally(() => setLoading(false));
  }, []);

  return (
    <div className="min-h-screen bg-gray-950 text-gray-100 flex items-start justify-center pt-16 px-4">
      <div className="w-full max-w-md bg-gray-900 rounded-2xl shadow-lg p-8">
        <h1 className="text-xl font-semibold mb-6 text-gray-50">
          info-service health
        </h1>

        {loading && (
          <p className="text-gray-400 text-sm">Checking info-service…</p>
        )}

        {!loading && error && (
          <div className="bg-red-950 border border-red-700 rounded-lg p-4 text-sm text-red-300">
            <span className="font-medium">Error:</span> {error}
          </div>
        )}

        {!loading && data && (
          <dl className="space-y-4 text-sm">
            <div className="flex justify-between">
              <dt className="text-gray-400">Status</dt>
              <dd className={data.ok ? 'text-green-400 font-medium' : 'text-red-400 font-medium'}>
                {data.ok ? 'ok' : 'degraded'}
              </dd>
            </div>
            <div className="flex justify-between">
              <dt className="text-gray-400">Uptime (s)</dt>
              <dd className="text-gray-100">{data.uptime.toFixed(1)}</dd>
            </div>
            <div className="flex justify-between">
              <dt className="text-gray-400">Collections</dt>
              <dd className="text-gray-100">
                {data.collections.length > 0 ? data.collections.join(', ') : 'none'}
              </dd>
            </div>
          </dl>
        )}
      </div>
    </div>
  );
}
