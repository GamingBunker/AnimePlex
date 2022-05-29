<template>
    <div class="row center">
        <div class="input-group" style="margin: 5px 0px;">
            <input v-model="value" type="text" class="form-control" placeholder="name..." style="width: 200px;">
        </div>
        <div class="btn-group" role="group" style="height: 40px">
            <button @click="searchInternalAll()" class="btn btn-warning" type="button" style="width: 33%;"><i class="bi bi-card-list"></i></button>
            <button @click="searchInternal()" class="btn btn-light" type="button" style="width: 33%;"><i class="bi bi-search"></i></button>
            <button @click="submitExternalAnime()" class="btn btn-danger" type="button" style="background-color: blueviolet; border-color: blueviolet; width: 33%;"><img src="../assets/img/logo_animesaturn.png" width="50px" style="object-fit: cover; margin-top: -2px;"></button>
            <button @click="submitExternalManga()" class="btn btn-danger" type="button" style="background-color: orange; border-color: orange; width: 33%;"><img src="../assets/img/MangaWorldLogo.svg" width="100%" style="height: auto; object-fit: cover; margin-top: -2px;"></button>
        </div>
    </div>
</template>

<script>
    export default {
        data(){
            return{
                value:""
            }
        },
        methods:{
            searchInternal(){
                $nuxt.$emit('search', this.value)
            },
            submitExternalAnime(){
                $nuxt.$emit('searchExternalAnime', this.value)
            },
            submitExternalManga(){
                $nuxt.$emit('searchExternalManga', this.value)
            },
            searchInternalAll(){
                $nuxt.$emit('searchall', this.value)
            }
        }
    }
</script>