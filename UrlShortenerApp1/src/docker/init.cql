CREATE KEYSPACE IF NOT EXISTS url_shortener
WITH replication = {'class': 'SimpleStrategy', 'replication_factor': 1};

CREATE TABLE IF NOT EXISTS url_shortener.urls (
    short_code text PRIMARY KEY,
    original_url text,
    creation_timestamp timestamp,
    expiration_date timestamp
);

CREATE TABLE IF NOT EXISTS url_shortener.url_analytics (
    short_code text,
    click_date timestamp,
    user_agent text,
    ip_address text,
    PRIMARY KEY (short_code, click_date)
) WITH CLUSTERING ORDER BY (click_date DESC);