-- Tables

create table if not exists users
(
    id       serial       not null primary key,

    username varchar(128) not null unique,
    email    varchar(128) not null unique,
    password bytea        not null,

    flags    int4         not null default 0 check (flags >= 0),

    created  timestamptz  not null default now()
);
