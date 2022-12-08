import axios from "axios";

export default {
    methods:{
        getAll() {
            this.isLoading = true;
            axios('/api/all')
                .then(res => {
                    const {data} = res;
                    this.data = data;
                })
                .catch(err => {
                    console.log(err)
                })
                .finally(() => {
                    this.isLoading = false;
                })
        },
        searchLocal() {
            axios(`/api/search-local?search=${this.search}`)
                .then(res => {
                    const {data} = res;
                    this.data = data;
                })
                .catch(err => {
                    console.log(err)
                })
                .finally(() => {
                    this.isLoading = false;
                })
        },
        searchAnime() {
            axios(`/api/search-animesaturn?search=${this.search}`)
                .then(res => {
                    const {data} = res;
                    this.data = data;
                })
                .catch(err => {
                    console.log(err)
                })
                .finally(() => {
                    this.isLoading = false;
                })
        },
        searchManga() {
            axios(`/api/search-mangaworld?search=${this.search}`)
                .then(res => {
                    const {data} = res;
                    this.data = data;
                })
                .catch(err => {
                    console.log(err)
                })
                .finally(() => {
                    this.isLoading = false;
                })
        }
    }
}