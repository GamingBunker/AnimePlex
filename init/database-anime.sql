CREATE DATABASE animePlex;

/*anime*/
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
    image text,
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

/*anime*/
CREATE TABLE manga
(
    name varchar(250) primary key not null,
    anime varchar(250) REFERENCES anime(name),
    author varchar(250),
    artist varchar(250),
    type varchar(250),
    description text,
    status boolean default true,
    totalVolumes integer,
    totalChapters integer,
    dateRelease integer,
    image text,
    urlPage text not null
);

CREATE TABLE chapter
(
    id varchar(500) primary key not null,
    nameManga varchar(250) REFERENCES manga(name) ON DELETE CASCADE not null,
    currentVolume integer not null,
    currentChapter decimal not null,
    pages varchar[],
    urlPage text not null,
    stateDownload varchar(100),
    percentualDownload int default 0
);

CREATE TABLE chapterRegister
(
    chapterId varchar(500) primary key REFERENCES chapter(id) ON DELETE CASCADE not null,
    chapterPath varchar(500),
    chapterHash varchar(64)
);