CREATE DATABASE animePlex;

CREATE TABLE anime
(
    name varchar(250) primary key not null,
    surname varchar(250) not null,
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
    id varchar(500) primary key not null,
    animeId  varchar(250) REFERENCES anime(name) ON DELETE CASCADE,
    numberEpisodeCurrent int not null,
    numberSeasonCurrent int default 1,
    stateDownload varchar(100),
    percentualDownload int default 0,

    urlVideo text default null,

    baseUrl varchar(250) default null,
    playlist varchar(250),
    resolution varchar(250),
    playlistSources varchar(250),
    startNumberFrame int,
    endNumberFrame int
);

CREATE TABLE episodeRegister
(
    episodeId varchar(500) primary key REFERENCES episode(id) ON DELETE CASCADE not null,
    episodePath varchar(500),
    episodeHash varchar(64)
);