import { defineStore } from 'pinia'

export const AnimeStore = defineStore({
  id:'anime',
  state: () => ({
    anime : [],
    index: [],
    CONST_MAX_INDEX:20,
    actualPage:0,
    selectAnime:null
  }),
  getters: {
    getAnime: (state) => {
      if(state.anime.length > 0)
        return state.anime.slice(state.index[state.actualPage].start, state.index[state.actualPage].end + 1)
      else
        return null
    },
    getIndex: (state) => state.index,
    getActualPage: (state) => state.actualPage,
    getSelectAnime: (state) => state.selectAnime,
  },
  actions: {
    insertAnime(insertAnime) {
      this.anime = insertAnime;
      this.index = []
      
      for(var pos=0; pos<this.anime.length; pos++)
      {
        if((pos+1)%this.CONST_MAX_INDEX == 0 || pos+1 == this.anime.length)
        {
          if(this.index.length <= 0)
          {
            this.index.push({
              'start':0,
              'end':pos
            })
          }else{
            this.index.push({
              'start':(this.index[this.index.length - 1].end + 1),
              'end':pos
            })
          }
        }
      }
    },
    nextPage(){
      if((this.actualPage + 1) < this.index.length )
        this.actualPage += 1
    },
    redoPage(){
      if((this.actualPage - 1) >= 0)
        this.actualPage -= 1
    },
    toPage(numberPositionPage){
      if(numberPositionPage > 0 && numberPositionPage <= this.index.length)
        this.actualPage = numberPositionPage - 1
    },
    select(anime){
      this.selectAnime = anime
    }
  }
})

export const MangaStore = defineStore({
  id:'manga',
  state: () => ({
    manga : [],
    index: [],
    CONST_MAX_INDEX:20,
    actualPage:0,
    selectManga:null
  }),
  getters: {
    getManga: (state) => {
      if(state.manga.length > 0)
        return state.manga.slice(state.index[state.actualPage].start, state.index[state.actualPage].end + 1)
      else
        return null
    },
    getIndex: (state) => state.index,
    getActualPage: (state) => state.actualPage,
    getSelectManga: (state) => state.selectManga,
  },
  actions: {
    insertManga(insertManga) {
      this.manga = insertManga;
      this.index = []
      
      for(var pos=0; pos<this.manga.length; pos++)
      {
        if((pos+1)%this.CONST_MAX_INDEX == 0 || pos+1 == this.manga.length)
        {
          if(this.index.length <= 0)
          {
            this.index.push({
              'start':0,
              'end':pos
            })
          }else{
            this.index.push({
              'start':(this.index[this.index.length - 1].end + 1),
              'end':pos
            })
          }
        }
      }
    },
    nextPage(){
      if((this.actualPage + 1) < this.index.length )
        this.actualPage += 1
    },
    redoPage(){
      if((this.actualPage - 1) >= 0)
        this.actualPage -= 1
    },
    toPage(numberPositionPage){
      if(numberPositionPage > 0 && numberPositionPage <= this.index.length)
        this.actualPage = numberPositionPage - 1
    },
    select(manga){
      this.selectManga = manga
    }
  }
})