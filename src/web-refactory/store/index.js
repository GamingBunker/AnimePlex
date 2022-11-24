import { defineStore } from 'pinia'

export const useStore = defineStore('store', {
    state: () => {
        return {
            currentSelectSearch:null,
        }
    },
    actions: {
        setCurrentSelectSearch(id){
            if(id === this.currentSelectSearch)
                this.currentSelectSearch = null;
            else
                this.currentSelectSearch = id;
        }
    },
    getters: {
        getCurrentSelectSearch(state){
            return state.currentSelectSearch;
        }
    },
})