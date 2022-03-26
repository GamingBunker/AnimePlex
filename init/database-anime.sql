CREATE DATABASE animePlex;

CREATE TABLE anime
(
    name varchar(250) primary key,
    studio varchar(250),
    description text,
    vote varchar(10),
    status boolean default true,
    durationEpisode varchar(100),
    episodeTotal int,
    dateRelease date,
    image bytea,
    urlpage text not null
);

CREATE TABLE episode
(
    id serial primary key,
    idAnime  varchar(250) REFERENCES anime(name),
    referer text not null,
    numberEpisodeCurrent int not null,
    numberSeasonCurrent int default 1,
    stateDownload varchar(100),
    percentualDownload int default -0,
    sizeFile int not null,

    urlVideo text default null,

    baseUrl varchar(250) default null,
    playlist varchar(250),
    resolution varchar(250),
    playlistSources varchar(250),
    startNumberFrame int,
    endNumberFrame int,

);