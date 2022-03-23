<template>
    <div style="width: 50%;">
        <label hidden>{{hide}}</label>
        <div class="card mb-3">
            <div class="card-header">
                <button @click="Close()" type="button" class="btn btn-primary"><i class="bi bi-arrow-left"></i></button>
                <template v-if="urlPageDownload">
                    <button @click="Download()" type="button" class="btn btn-danger"><i class="bi bi-cloud-download"></i></button>
                </template>
            </div>
            <div class="card-body">
                <h5 class="card-title">{{name}}</h5>
                <p class="card-text">{{description}}</p>
                <p class="card-text"><small class="text-muted"></small></p>
                <div v-for="episode in episodes" :key="episode.id">
                    {{episode.idAnime}} episode: {{episode.numberEpisodeCurrent}}
                    <template v-if="episode.stateDownload == 'downloading'">
                        <div class="progress">
                            <div class="progress-bar progress-bar-striped progress-bar-animated" role="progressbar" :style="'width: '+episode.percentualDownload+'%'" :aria-valuenow="episode.percentualDownload" aria-valuemin="0" aria-valuemax="100">{{episode.percentualDownload}}%</div>
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
        <template v-if="urlExternal == false">
            <img :src="'data:image/jpg;base64,'+ConvertBase64(image)" class="card-img-top">
        </template>
        <template v-else>
            <img :src="image" class="card-img-top">
        </template>
        </div>
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
        totalEpisode:String,
        urlPageDownload:String,
        urlExternal:Boolean
    },

    methods: {
        ConvertBase64(imgBase64){
            var buff = new Buffer(imgBase64);
            return  buff.toString();
        },
        Close(){
            $nuxt.$emit('close', this.name);
        },
        Download(){
            //get api external
            this.$axios.post("https://localhost:44300/animesaturn/download", JSON.stringify({Url: this.urlPageDownload}),
            {
                headers: {
                    'Content-Type': 'application/json'
                }
            })
            .then(rs => {
                if(rs.status == 201){
                    this.Close()
                    $nuxt.$emit("reloadViewDetails", rs.data);
                }else{
                    console.log('error download');
                }
            });
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