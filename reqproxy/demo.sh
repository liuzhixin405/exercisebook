#!/usr/bin/env bash
# Walks through the gateway -> backend chain with curl. Start the three services first (README).
G=http://localhost:5100
B=http://localhost:5200
JAR=$(mktemp)
J='Content-Type: application/json'
step() { printf '\n== %s\n' "$1"; }
show() { curl -s -w '  [HTTP %{http_code}]\n' "$@"; }

step "1. Not signed in: the gateway refuses, the backend is never reached"
show $G/api/regions/hk/sources

step "2. Sign in as a viewer (the cookie goes to the cookie jar)"
show -c $JAR -H "$J" -d '{"email":"viewer@demo.local","password":"viewer-pass"}' $G/login

step "3. Read through the gateway: /api/regions/hk/sources -> backend /api/sources"
show -b $JAR $G/api/regions/hk/sources

step "4. whoami: what the BACKEND saw (no cookie, the gateway's key, the person in headers)"
show -b $JAR $G/api/regions/hk/whoami

step "5. Forged identity: the client sends X-Demo-Role: technical; the gateway removes it"
show -b $JAR -H 'X-Demo-Role: technical' -H 'X-Demo-User: boss@demo.local' $G/api/regions/hk/whoami

step "6. A viewer tries to save: the gateway lets it through (it only checks 'signed in'), the backend says 403"
show -b $JAR -X PUT -H "$J" -d '{"depth":20,"expectedVersion":1}' $G/api/regions/hk/sources/Binance

step "7. Sign in as ops (role operation): can change depth, not the connector"
show -c $JAR -H "$J" -d '{"email":"ops@demo.local","password":"ops-pass"}' $G/login
show -b $JAR -X PUT -H "$J" -d '{"depth":20,"expectedVersion":1}' $G/api/regions/hk/sources/Binance
show -b $JAR -X PUT -H "$J" -d '{"depth":20,"expectedVersion":2,"connector":"okx"}' $G/api/regions/hk/sources/Binance

step "8. Same edit with a stale version: 409 and the current state"
show -b $JAR -X PUT -H "$J" -d '{"depth":30,"expectedVersion":1}' $G/api/regions/hk/sources/Binance

step "9. Each region has its own backend and key: ny still has depth 10"
show -b $JAR $G/api/regions/ny/sources/Binance

step "10. What is not on the gateway's list is not forwarded: another method (405), another path (404)"
show -b $JAR -X DELETE $G/api/regions/hk/sources/Binance
show -b $JAR $G/api/regions/hk/keys

step "11. Calling the backend directly: no key, a cookie, and a wrong key all fail (401)"
show $B/api/sources
show -b $JAR $B/api/sources
show -H 'Authorization: ApiKey wrong' -H 'X-Demo-User: a@b' -H 'X-Demo-Role: technical' $B/api/sources

rm -f $JAR
