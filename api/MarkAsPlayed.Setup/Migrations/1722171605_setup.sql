﻿--AUTHOR SECTION
CREATE TABLE author (
    author_id INTEGER not null PRIMARY KEY generated by default AS identity,
    firebase_id text not null UNIQUE,
    name TEXT not null UNIQUE,
    description_pl TEXT not null,
    description_en TEXT not null
);

--ARTICLE SECTION
CREATE TABLE article_type (
    article_type_id INTEGER not null PRIMARY KEY generated by default AS identity,
    name TEXT not null UNIQUE,
    group_sign CHAR not null
);

CREATE TABLE article (
    article_id BIGINT not null PRIMARY KEY generated by default AS identity,
    article_type_id INTEGER not null REFERENCES article_type (article_type_id),
    title TEXT not null,
    intro TEXT not null,
    created timestamptz not null DEFAULT NOW(),
    modified timestamptz not null DEFAULT NOW(),
    overwrite_creation_time timestamptz NOT NULL DEFAULT '-infinity',
    created_by_author_id INTEGER not null REFERENCES author (author_id),
    properties JSONB not null
);

CREATE TABLE article_content (
    article_id BIGINT not null REFERENCES article (article_id) ON DELETE CASCADE,
    content TEXT not null
);

--GAMING PLATFORM SECTION
CREATE TABLE gaming_platform (
    gaming_platform_id INTEGER not null PRIMARY KEY generated by default AS identity,
    name TEXT not null UNIQUE,
    group_sign CHAR not null
);

CREATE TABLE article_gaming_platform (
    article_id BIGINT not null REFERENCES article (article_id) ON DELETE CASCADE,
    gaming_platform_id INTEGER not null REFERENCES gaming_platform (gaming_platform_id),
        PRIMARY KEY (article_id, gaming_platform_id)
);
CREATE INDEX gaming_platform_id_idx ON gaming_platform (gaming_platform_id);

CREATE INDEX article_gaming_platform_gaming_platform_id_idx ON article_gaming_platform (gaming_platform_id);

--ARTICLE IMAGE SECTION
CREATE TABLE article_image (
    article_image_id INTEGER not null PRIMARY KEY generated by default AS identity,
    article_id BIGINT not null REFERENCES article (article_id) ON DELETE CASCADE,
    path TEXT not null,
    external_path TEXT not null,
    is_active boolean not null DEFAULT true
);
CREATE INDEX article_image_id_idx ON article_image (article_image_id);

--TAG SECTION
CREATE TABLE tag (
	tag_id INTEGER not null PRIMARY KEY generated by default AS identity,
    name text not null UNIQUE,
    group_sign CHAR not null
);

CREATE TABLE article_tag (
    article_id INTEGER not null REFERENCES article (article_id) ON DELETE CASCADE,
    tag_id INTEGER not null REFERENCES tag (tag_id),
    is_active boolean not null,
    PRIMARY KEY (article_id, tag_id)
);
CREATE INDEX tag_id_idx ON tag (tag_id);

CREATE INDEX article_tag_tag_id_idx ON article_tag (tag_id);

--ARTICLE HISTORY SECTION
CREATE TABLE article_version_history (
    article_id BIGINT not null REFERENCES article (article_id) ON DELETE CASCADE,
    transaction_id TEXT not null UNIQUE,
    created_at timestamptz not null DEFAULT NOW(),
    created_by_author_id INTEGER not null REFERENCES author (author_id),
    properties JSONB not null
);

--ERROR SECTION
CREATE TABLE error (
    error_id BIGINT not null PRIMARY KEY generated by default AS identity,
    log_level TEXT not null,
    thread_id TEXT not null,
    event_id TEXT not null,
    event_name TEXT not null,
    message TEXT not null,
    exception_message TEXT not null,
    exception_stack_trace TEXT not null,
    exception_source TEXT not null
);

--FIELDS SECTION
CREATE TABLE field (
    field_id INTEGER not null PRIMARY KEY generated by default AS identity,
    name TEXT not null UNIQUE,
    type TEXT not null,
    attributes TEXT not null,
    connections TEXT not null
);
CREATE INDEX field_id_idx ON field (field_id);