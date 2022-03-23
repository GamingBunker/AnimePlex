<template>
    <div class="card mb-3" style="width: 50%;">
        <label hidden>{{hide}}</label>
        <div class="card-body">
            <h5 class="card-title">{{name}}</h5>
            <p class="card-text">{{description}}</p>
            <p class="card-text"><small class="text-muted">Last updated 3 mins ago</small></p>
            <div v-for="episode in episodes" :key="episode.id">
                {{episode.idAnime}}-s{{episode.numberSeasonCurrent}}e{{episode.numberEpisodeCurrent}}
                <template v-if="episode.stateDownload == 'downloading'">
                    <div class="progress">
                        <div class="progress-bar" role="progressbar" :style="'width: '+episode.percentualDownload+'%'" :aria-valuenow="episode.percentualDownload" aria-valuemin="0" aria-valuemax="100">{{episode.percentualDownload}}%</div>
                    </div>
                </template>
                <template v-else-if="episode.stateDownload == 'completed'">
                    <div class="progress">
                        <div class="progress-bar bg-success" role="progressbar" :style="'width: '+episode.percentualDownload+'%'" :aria-valuenow="episode.percentualDownload" aria-valuemin="0" aria-valuemax="100">{{episode.percentualDownload}}%</div>
                    </div>
                </template>
                <template v-else-if="episode.stateDownload == 'failed'">
                    <div class="progress">
                        <div class="progress-bar bg-danger" role="progressbar" :style="'width: '+episode.percentualDownload+'%'" :aria-valuenow="episode.percentualDownload" aria-valuemin="0" aria-valuemax="100">{{episode.percentualDownload}}%</div>
                    </div>
                </template>
                <template v-else-if="episode.stateDownload == 'pending'">
                    <div class="progress">
                        <div class="progress-bar bg-warning" role="progressbar" :style="'width: 100%'" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100">pending</div>
                    </div>
                </template>
            </div>
        </div>
    <img :src="'data:image/jpg;base64,'+ConvertBase64(image)" class="card-img-top">
    </div>
</template>

<script>
    export default {
        data(){
            return{
                timer:1000,
                hide:"",
                episodes:[]
            }
        },
        props:{
            name:String,
            description:String,
            date:Date,
            vote:String,
            studio:String,
            image:String,
            status:Boolean,
            urlPage:String,
            duration:String,
            totalEpisode:String

        },
        methods: {
            ConvertBase64(imgBase64){
                var buff = new Buffer(imgBase64);
                return  buff.toString()
            }
        },
        watch:{
            hide:{
                handler(){
                    //get api internal
                    this.$axios.get("https://localhost:44300/episode/name/" + this.name)
                        .then(rs => {
                        this.episodes = rs.data
                    });

                    setTimeout(() => {
                        this.hide = new Date();
                    }, 3000);
                },
                immediate: true
            }
        }
    }
</script>

<style lang="scss" scoped>

</style>