create table anime
(
    name varchar(250) primary key,
    author varchar(250),
    description text,
    vote varchar(10),
    status boolean default true,
    durationEpisode varchar(100),
    episodeTotal int,
    dateRelease date,
    image bytea,
    urlpage text not null
);

create table episode
(
    id serial primary key,
    idAnime  varchar(250) REFERENCES anime(name),
    urlVideo text not null,
    referer text not null,
    numberEpisodeCurrent int not null
);