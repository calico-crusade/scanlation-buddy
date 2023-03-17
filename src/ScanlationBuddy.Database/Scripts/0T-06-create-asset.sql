CREATE TABLE IF NOT EXISTS buddy_asset (
	id INTEGER PRIMARY KEY,

	file_id INTEGER
	replaced_by INTEGER,
	replaced_with INTEGER,
	name TEXT NOT NULL,
	description TEXT NOT NULL,
	creator_id INTEGER NOT NULL,

	created_at TEXT NOT NULL,
	updated_at TEXT NOT NULL,
	deleted_at TEXT,

	FOREIGN KEY(creator_id) REFERENCES buddy_user(id),
	FOREIGN KEY(file_id) REFERENCES buddy_file(id),
	FOREIGN KEY(replaced_by) REFERENCES buddy_asset(id),
	FOREIGN KEY(replaced_with) REFERENCES buddy_asset(id),

	UNIQUE (name, deleted_at)
)