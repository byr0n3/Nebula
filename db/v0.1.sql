-- region Types

do
$$
    begin

        if not exists (select * from pg_type where typname = 'shipment_source') then
            create type shipment_source as enum ('postnl', 'dhl');
        end if;

        if not exists (select * from pg_type where typname = 'shipment_state') then
            create type shipment_state as enum ('registered', 'received', 'sorted', 'out_for_delivery', 'delivered');
        end if;

    end
$$;

-- endregion

-- region Tables

create table if not exists users
(
    id       serial       not null primary key,

    username varchar(128) not null unique,
    email    varchar(128) not null unique,
    password bytea        not null,

    flags    int4         not null default 0 check (flags >= 0),

    created  timestamptz  not null default now()
);

create table if not exists shipments
(
    id        serial          not null primary key,
    code      varchar(128)    not null,
    source    shipment_source not null,
    zip_code  varchar(8)      not null,

    state     shipment_state  not null,
    eta       tstzrange,
    arrived   timestamptz,

    recipient varchar(128),
    sender    varchar(128),

    created   timestamptz     not null,
    updated   timestamptz     not null,

    unique (code, source)
);

create table if not exists users_shipments
(
    user_id     int4 not null references users,
    shipment_id int4 not null references shipments,

    unique (user_id, shipment_id)
);

create table if not exists users_push_subscriptions
(
    id       serial      not null primary key,
    user_id  int4        not null references users,

    endpoint text        not null unique,
    expires  timestamptz,
    p256dh   text        not null,
    auth     text        not null,

    created  timestamptz not null default now()
);

-- endregion
