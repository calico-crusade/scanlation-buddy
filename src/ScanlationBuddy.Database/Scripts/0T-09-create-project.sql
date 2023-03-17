CREATE TABLE IF NOT EXISTS buddy_project (
	id INTEGER PRIMARY KEY,

	hash TEXT NOT NULL,
	title TEXT NOT NULL,
	notes TEXT NOT NULL,
	manga_dex_id TEXT NOT NULL,
	state INTEGER NOT NULL,
	creator_id NOT NULL,

	created_at TEXT NOT NULL,
	updated_at TEXT NOT NULL,
	deleted_at TEXT,

	FOREIGN KEY(creator_id) REFERENCES buddy_user(id)
)