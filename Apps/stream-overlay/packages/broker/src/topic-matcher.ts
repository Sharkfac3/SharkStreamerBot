/**
 * Returns true when a subscription pattern matches a topic string.
 *
 * Two forms are supported:
 *   - Exact:    "lotat.session.start"  →  matches that exact topic only
 *   - Wildcard: "lotat.*"             →  matches any topic starting with "lotat."
 *
 * Wildcards are trailing only. "lotat.*" matches "lotat.session.start" and
 * "lotat.vote.cast". Mid-string wildcards ("lotat.*.start") are not supported
 * and will not match anything beyond an exact string comparison.
 */
export function topicMatches(subscription: string, topic: string): boolean {
  if (subscription === topic) return true;

  if (subscription.endsWith(".*")) {
    // "lotat.*" → prefix is "lotat." — topic must start with that
    const prefix = subscription.slice(0, -1); // removes the "*", keeps the "."
    return topic.startsWith(prefix);
  }

  return false;
}
