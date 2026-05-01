import { useEffect, useMemo, useState } from 'react';
import type { ReactNode } from 'react';

const INFO_SERVICE_BASE = 'http://127.0.0.1:8766';

type PendingIntroStatus = 'pending' | 'fulfilled' | 'rejected';

interface PendingIntroRecord {
  userId: string;
  userLogin: string;
  redeemId: string;
  redeemUtc: number;
  rewardTitle: string;
  userInput?: string;
  status: PendingIntroStatus;
  resolvedUtc?: number;
}

interface UserIntroRecord {
  userId: string;
  userLogin: string;
  soundFile?: string;
  gifFile?: string;
  enabled: boolean;
  updatedUtc: number;
}

interface CollectionEnvelope<TRecord> {
  schemaVersion: number;
  collection: string;
  updatedUtc: number;
  records: Record<string, TRecord>;
}

interface FulfillForm {
  soundFile: string;
  gifFile: string;
  enabled: boolean;
}

const EMPTY_FULFILL_FORM: FulfillForm = {
  soundFile: '',
  gifFile: '',
  enabled: true,
};

export default function PendingIntrosPage() {
  const [records, setRecords] = useState<PendingIntroRecord[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | undefined>();
  const [fulfillTarget, setFulfillTarget] = useState<PendingIntroRecord | null>(null);
  const [form, setForm] = useState({ ...EMPTY_FULFILL_FORM });
  const [formErrors, setFormErrors] = useState<{ soundFile?: string }>({});
  const [saving, setSaving] = useState(false);

  function loadRecords() {
    setLoading(true);
    setError(undefined);
    fetch(`${INFO_SERVICE_BASE}/info/pending-intros`)
      .then((res) => {
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        return res.json() as Promise<CollectionEnvelope<PendingIntroRecord>>;
      })
      .then((env) => {
        setRecords(Object.values(env.records));
      })
      .catch((err: unknown) =>
        setError(err instanceof Error ? err.message : String(err))
      )
      .finally(() => setLoading(false));
  }

  useEffect(() => {
    loadRecords();
  }, []);

  const sortedRecords = useMemo(
    () =>
      [...records].sort((a, b) => {
        if (a.status === 'pending' && b.status !== 'pending') return -1;
        if (a.status !== 'pending' && b.status === 'pending') return 1;
        return b.redeemUtc - a.redeemUtc;
      }),
    [records]
  );

  const pendingCount = records.filter((r) => r.status === 'pending').length;

  function openFulfill(record: PendingIntroRecord) {
    setFulfillTarget(record);
    setForm({ ...EMPTY_FULFILL_FORM });
    setFormErrors({});
    setError(undefined);
  }

  function closeFulfill() {
    setFulfillTarget(null);
    setForm({ ...EMPTY_FULFILL_FORM });
    setFormErrors({});
  }

  function validateFulfillForm(): boolean {
    const errs: { soundFile?: string } = {};
    if (!form.soundFile.trim()) errs.soundFile = 'Required';
    setFormErrors(errs);
    return Object.keys(errs).length === 0;
  }

  async function handleFulfill() {
    if (!fulfillTarget || !validateFulfillForm()) return;
    setSaving(true);
    setError(undefined);

    const now = Date.now();
    const userIntroPayload: UserIntroRecord = {
      userId: fulfillTarget.userId,
      userLogin: fulfillTarget.userLogin,
      soundFile: form.soundFile.trim(),
      gifFile: form.gifFile.trim() || undefined,
      enabled: form.enabled,
      updatedUtc: now,
    };

    const pendingPayload: PendingIntroRecord = {
      ...fulfillTarget,
      status: 'fulfilled',
      resolvedUtc: now,
    };

    try {
      const userIntroRes = await fetch(
        `${INFO_SERVICE_BASE}/info/user-intros/${fulfillTarget.userId}`,
        {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(userIntroPayload),
        }
      );
      if (!userIntroRes.ok) throw new Error(`user-intros HTTP ${userIntroRes.status}`);

      const pendingRes = await fetch(
        `${INFO_SERVICE_BASE}/info/pending-intros/${fulfillTarget.redeemId}`,
        {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(pendingPayload),
        }
      );
      if (!pendingRes.ok) throw new Error(`pending-intros HTTP ${pendingRes.status}`);

      closeFulfill();
      loadRecords();
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : String(err));
    } finally {
      setSaving(false);
    }
  }

  async function handleReject(record: PendingIntroRecord) {
    if (!window.confirm(`Reject pending intro from ${record.userLogin} (${record.redeemId})?`)) return;
    setSaving(true);
    setError(undefined);

    const payload: PendingIntroRecord = {
      ...record,
      status: 'rejected',
      resolvedUtc: Date.now(),
    };

    try {
      const res = await fetch(`${INFO_SERVICE_BASE}/info/pending-intros/${record.redeemId}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload),
      });
      if (!res.ok) throw new Error(`HTTP ${res.status}`);
      loadRecords();
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : String(err));
    } finally {
      setSaving(false);
    }
  }

  return (
    <div className="min-h-screen bg-gray-950 text-gray-100 px-6 py-10">
      <div className="max-w-7xl mx-auto">
        <div className="flex items-start justify-between gap-4 mb-6">
          <div>
            <h1 className="text-xl font-semibold text-gray-50">Pending Intros</h1>
            <p className="text-sm text-gray-400 mt-1">
              Review captured redemptions, then fulfill or reject pending records.
            </p>
          </div>
          <div className="text-sm text-gray-300 bg-gray-900 border border-gray-800 rounded-lg px-4 py-2">
            Pending: <span className="font-semibold text-yellow-300">{pendingCount}</span>
          </div>
        </div>

        {error && (
          <div className="bg-red-950 border border-red-700 rounded-lg p-4 text-sm text-red-300 mb-6">
            <span className="font-medium">Error:</span> {error}
          </div>
        )}

        {loading ? (
          <p className="text-gray-400 text-sm">Loading…</p>
        ) : sortedRecords.length === 0 ? (
          <p className="text-gray-500 text-sm">No pending-intros records yet.</p>
        ) : (
          <div className="overflow-x-auto rounded-xl border border-gray-800">
            <table className="w-full text-sm text-left">
              <thead className="bg-gray-900 text-gray-400 uppercase text-xs tracking-wide">
                <tr>
                  <th className="px-4 py-3">status</th>
                  <th className="px-4 py-3">userLogin</th>
                  <th className="px-4 py-3">userId</th>
                  <th className="px-4 py-3">redeemId</th>
                  <th className="px-4 py-3">userInput</th>
                  <th className="px-4 py-3">rewardTitle</th>
                  <th className="px-4 py-3">redeemUtc</th>
                  <th className="px-4 py-3">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-800">
                {sortedRecords.map((r) => (
                  <tr
                    key={r.redeemId}
                    className={
                      r.status === 'pending'
                        ? 'bg-yellow-950/30 hover:bg-yellow-950/50 transition-colors'
                        : 'bg-gray-900 hover:bg-gray-800 transition-colors opacity-80'
                    }
                  >
                    <td className="px-4 py-3"><StatusBadge status={r.status} /></td>
                    <td className="px-4 py-3 text-gray-100">{r.userLogin}</td>
                    <td className="px-4 py-3 font-mono text-gray-300">{r.userId}</td>
                    <td className="px-4 py-3 font-mono text-gray-400">{r.redeemId}</td>
                    <td className="px-4 py-3 text-gray-300 max-w-xs whitespace-pre-wrap">
                      {r.userInput || '—'}
                    </td>
                    <td className="px-4 py-3 text-gray-400">{r.rewardTitle}</td>
                    <td className="px-4 py-3 text-gray-400">{formatDate(r.redeemUtc)}</td>
                    <td className="px-4 py-3">
                      {r.status === 'pending' ? (
                        <div className="flex gap-3">
                          <button
                            onClick={() => openFulfill(r)}
                            disabled={saving}
                            className="text-indigo-400 hover:text-indigo-300 disabled:opacity-50 transition-colors"
                          >
                            Fulfill
                          </button>
                          <button
                            onClick={() => handleReject(r)}
                            disabled={saving}
                            className="text-red-400 hover:text-red-300 disabled:opacity-50 transition-colors"
                          >
                            Reject
                          </button>
                        </div>
                      ) : (
                        <span className="text-xs text-gray-500">
                          Resolved {r.resolvedUtc ? formatDate(r.resolvedUtc) : '—'}
                        </span>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {fulfillTarget && (
        <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50 px-4">
          <div className="bg-gray-900 border border-gray-700 rounded-2xl shadow-2xl w-full max-w-lg p-8">
            <h2 className="text-lg font-semibold text-gray-50 mb-2">Fulfill Pending Intro</h2>
            <p className="text-sm text-gray-400 mb-6">
              Creates or updates the user-intros record for {fulfillTarget.userLogin} and marks this redeem fulfilled.
            </p>

            <div className="space-y-4">
              <ReadOnlyField label="userId" value={fulfillTarget.userId} />
              <ReadOnlyField label="userLogin" value={fulfillTarget.userLogin} />

              <Field label="soundFile *">
                <input
                  type="text"
                  value={form.soundFile}
                  onChange={(e) => setForm((f) => ({ ...f, soundFile: e.target.value }))}
                  className="input-base"
                  placeholder="alice.mp3"
                />
                {formErrors.soundFile && <FieldError msg={formErrors.soundFile} />}
              </Field>

              <Field label="gifFile">
                <input
                  type="text"
                  value={form.gifFile}
                  onChange={(e) => setForm((f) => ({ ...f, gifFile: e.target.value }))}
                  className="input-base"
                  placeholder="alice.gif"
                />
              </Field>

              <Field label="enabled">
                <label className="flex items-center gap-2 cursor-pointer">
                  <input
                    type="checkbox"
                    checked={form.enabled}
                    onChange={(e) => setForm((f) => ({ ...f, enabled: e.target.checked }))}
                    className="w-4 h-4 rounded accent-indigo-500"
                  />
                  <span className="text-sm text-gray-300">Active immediately</span>
                </label>
              </Field>
            </div>

            <div className="flex justify-end gap-3 mt-6">
              <button
                onClick={closeFulfill}
                disabled={saving}
                className="px-4 py-2 text-sm text-gray-400 hover:text-gray-200 disabled:opacity-50 transition-colors"
              >
                Cancel
              </button>
              <button
                onClick={handleFulfill}
                disabled={saving}
                className="bg-indigo-600 hover:bg-indigo-500 disabled:opacity-50 text-white text-sm font-medium px-5 py-2 rounded-lg transition-colors"
              >
                {saving ? 'Saving…' : 'Fulfill'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

function StatusBadge({ status }: { status: PendingIntroStatus }) {
  const className =
    status === 'pending'
      ? 'bg-yellow-500/15 text-yellow-300 border-yellow-700'
      : status === 'fulfilled'
        ? 'bg-green-500/15 text-green-300 border-green-700'
        : 'bg-red-500/15 text-red-300 border-red-700';

  return (
    <span className={`inline-flex rounded-full border px-2 py-0.5 text-xs font-medium ${className}`}>
      {status}
    </span>
  );
}

function ReadOnlyField({ label, value }: { label: string; value: string }) {
  return (
    <Field label={label}>
      <div className="font-mono text-sm text-gray-300 bg-gray-950 border border-gray-800 rounded-lg px-3 py-2">
        {value}
      </div>
    </Field>
  );
}

function Field({ label, children }: { label: string; children: ReactNode }) {
  return (
    <div>
      <label className="block text-xs text-gray-400 mb-1">{label}</label>
      {children}
    </div>
  );
}

function FieldError({ msg }: { msg: string }) {
  return <p className="text-xs text-red-400 mt-1">{msg}</p>;
}

function formatDate(value: number) {
  return new Date(value).toLocaleString();
}
