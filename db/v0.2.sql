alter table users
    alter culture set default 'nl-NL',
    add if not exists ui_culture varchar(8)  not null default 'nl-NL',
    add if not exists timezone   varchar(32) not null default 'Europe/Amsterdam';
