import { useEffect, useState } from 'react';

const INFO_SERVICE_BASE = 'http://127.0.0.1:8766';

interface UserIntroRecord {
  userId: string;
  userLogin: string;
  soundFile?: string;
  gifFile?: string;
  enabled: boolean;
  notes?: string;
  updatedUtc: number;
}

interface CollectionEnvelope {
  schemaVersion: number;
  collection: string;
  updatedUtc: number;
  records: Record<string, UserIntroRecord>;
}

const EMPTY_FORM: Omit<UserIntroRecord, 'updatedUtc'> = {
  userId: '',
  userLogin: '',
  soundFile: '',
  gifFile: '',
  enabled: true,
  notes: '',
};

export default function UserIntrosPage() {
  const [records, setRecords] = useState<UserIntroRecord[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | undefined>();
  const [formOpen, setFormOpen] = useState(false);
  const [editTarget, setEditTarget] = useState<UserIntroRecord | null>(null);
  const [form, setForm] = useState({ ...EMPTY_FORM });
  const [formErrors, setFormErrors] = useState<{ userId?: string; userLogin?: string }>({});
  const [saving, setSaving] = useState(false);

  function loadRecords() {
    setLoading(true);
    setError(undefined);
    fetch(`${INFO_SERVICE_BASE}/info/user-intros`)
      .then((res) => {
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        return res.json() as Promise<CollectionEnvelope>;
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

  function openCreate() {
    setEditTarget(null);
    setForm({ ...EMPTY_FORM });
    setFormErrors({});
    setFormOpen(true);
  }

  function openEdit(record: UserIntroRecord) {
    setEditTarget(record);
    setForm({
      userId: record.userId,
      userLogin: record.userLogin,
      soundFile: record.soundFile ?? '',
      gifFile: record.gifFile ?? '',
      enabled: record.enabled,
      notes: record.notes ?? '',
    });
    setFormErrors({});
    setFormOpen(true);
  }

  function closeForm() {
    setFormOpen(false);
    setEditTarget(null);
    setFormErrors({});
  }

  function validateForm(): boolean {
    const errs: { userId?: string; userLogin?: string } = {};
    if (!form.userId.trim()) errs.userId = 'Required';
    if (!form.userLogin.trim()) errs.userLogin = 'Required';
    setFormErrors(errs);
    return Object.keys(errs).length === 0;
  }

  function handleSubmit() {
    if (!validateForm()) return;
    setSaving(true);

    const payload: UserIntroRecord = {
      userId: form.userId.trim(),
      userLogin: form.userLogin.trim(),
      soundFile: form.soundFile?.trim() || undefined,
      gifFile: form.gifFile?.trim() || undefined,
      enabled: form.enabled,
      notes: form.notes?.trim() || undefined,
      updatedUtc: Date.now(),
    };

    const method = editTarget ? 'PUT' : 'POST';
    const url = `${INFO_SERVICE_BASE}/info/user-intros/${payload.userId}`;

    fetch(url, {
      method,
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    })
      .then((res) => {
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        closeForm();
        loadRecords();
      })
      .catch((err: unknown) =>
        setError(err instanceof Error ? err.message : String(err))
      )
      .finally(() => setSaving(false));
  }

  function handleDelete(record: UserIntroRecord) {
    if (!window.confirm(`Delete intro for ${record.userLogin} (${record.userId})?`)) return;
    fetch(`${INFO_SERVICE_BASE}/info/user-intros/${record.userId}`, { method: 'DELETE' })
      .then((res) => {
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        setRecords((prev) => prev.filter((r) => r.userId !== record.userId));
      })
      .catch((err: unknown) =>
        setError(err instanceof Error ? err.message : String(err))
      );
  }

  return (
    <div className="min-h-screen bg-gray-950 text-gray-100 px-6 py-10">
      <div className="max-w-5xl mx-auto">
        <div className="flex items-center justify-between mb-6">
          <h1 className="text-xl font-semibold text-gray-50">User Intros</h1>
          <button
            onClick={openCreate}
            className="bg-indigo-600 hover:bg-indigo-500 text-white text-sm font-medium px-4 py-2 rounded-lg transition-colors"
          >
            + Add Record
          </button>
        </div>

        {error && (
          <div className="bg-red-950 border border-red-700 rounded-lg p-4 text-sm text-red-300 mb-6">
            <span className="font-medium">Error:</span> {error}
          </div>
        )}

        {loading ? (
          <p className="text-gray-400 text-sm">Loading…</p>
        ) : records.length === 0 ? (
          <p className="text-gray-500 text-sm">No records yet. Add the first one.</p>
        ) : (
          <div className="overflow-x-auto rounded-xl border border-gray-800">
            <table className="w-full text-sm text-left">
              <thead className="bg-gray-900 text-gray-400 uppercase text-xs tracking-wide">
                <tr>
                  <th className="px-4 py-3">userId</th>
                  <th className="px-4 py-3">userLogin</th>
                  <th className="px-4 py-3">soundFile</th>
                  <th className="px-4 py-3">gifFile</th>
                  <th className="px-4 py-3">enabled</th>
                  <th className="px-4 py-3">updatedUtc</th>
                  <th className="px-4 py-3">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-800">
                {records.map((r) => (
                  <tr key={r.userId} className="bg-gray-900 hover:bg-gray-800 transition-colors">
                    <td className="px-4 py-3 font-mono text-gray-300">{r.userId}</td>
                    <td className="px-4 py-3 text-gray-100">{r.userLogin}</td>
                    <td className="px-4 py-3 text-gray-400">{r.soundFile ?? '—'}</td>
                    <td className="px-4 py-3 text-gray-400">{r.gifFile ?? '—'}</td>
                    <td className="px-4 py-3">
                      <span className={r.enabled ? 'text-green-400' : 'text-gray-500'}>
                        {r.enabled ? 'Yes' : 'No'}
                      </span>
                    </td>
                    <td className="px-4 py-3 text-gray-400">
                      {new Date(r.updatedUtc).toLocaleString()}
                    </td>
                    <td className="px-4 py-3 flex gap-3">
                      <button
                        onClick={() => openEdit(r)}
                        className="text-indigo-400 hover:text-indigo-300 transition-colors"
                      >
                        Edit
                      </button>
                      <button
                        onClick={() => handleDelete(r)}
                        className="text-red-400 hover:text-red-300 transition-colors"
                      >
                        Delete
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {formOpen && (
        <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50 px-4">
          <div className="bg-gray-900 border border-gray-700 rounded-2xl shadow-2xl w-full max-w-lg p-8">
            <h2 className="text-lg font-semibold text-gray-50 mb-6">
              {editTarget ? 'Edit Intro' : 'Add Intro'}
            </h2>

            <div className="space-y-4">
              <Field label="userId *">
                <input
                  type="text"
                  value={form.userId}
                  disabled={!!editTarget}
                  onChange={(e) => setForm((f) => ({ ...f, userId: e.target.value }))}
                  className="input-base disabled:opacity-50 disabled:cursor-not-allowed"
                  placeholder="12345"
                />
                {formErrors.userId && <FieldError msg={formErrors.userId} />}
              </Field>

              <Field label="userLogin *">
                <input
                  type="text"
                  value={form.userLogin}
                  onChange={(e) => setForm((f) => ({ ...f, userLogin: e.target.value }))}
                  className="input-base"
                  placeholder="alice"
                />
                {formErrors.userLogin && <FieldError msg={formErrors.userLogin} />}
              </Field>

              <Field label="soundFile">
                <input
                  type="text"
                  value={form.soundFile}
                  onChange={(e) => setForm((f) => ({ ...f, soundFile: e.target.value }))}
                  className="input-base"
                  placeholder="alice.mp3"
                />
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
                  <span className="text-sm text-gray-300">Active</span>
                </label>
              </Field>

              <Field label="notes">
                <textarea
                  value={form.notes}
                  onChange={(e) => setForm((f) => ({ ...f, notes: e.target.value }))}
                  className="input-base resize-none h-20"
                  placeholder="Optional notes…"
                />
              </Field>
            </div>

            <div className="flex justify-end gap-3 mt-6">
              <button
                onClick={closeForm}
                className="px-4 py-2 text-sm text-gray-400 hover:text-gray-200 transition-colors"
              >
                Cancel
              </button>
              <button
                onClick={handleSubmit}
                disabled={saving}
                className="bg-indigo-600 hover:bg-indigo-500 disabled:opacity-50 text-white text-sm font-medium px-5 py-2 rounded-lg transition-colors"
              >
                {saving ? 'Saving…' : editTarget ? 'Save Changes' : 'Create'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

function Field({ label, children }: { label: string; children: React.ReactNode }) {
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
