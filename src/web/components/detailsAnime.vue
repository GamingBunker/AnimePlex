<template>
    <div style="width: 40%;">
        <!-- auto refresh api  -->
        <label hidden>{{hide}}</label>
        
        <!-- message error  -->
        <template v-if="conflict">
            <div class="alert alert-danger alert-dismissible">
                <strong>This anime already exists!</strong> Try press button 'Search'
                <button @click="closeAlert()" type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            </div>
        </template>
        <!-- message success download  -->
        <template v-else-if="success">
            <div class="alert alert-success alert-dismissible">
                Success download information! Soon the downloads will start.
                <button @click="closeAlert()" type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            </div>
        </template>

        <!-- card  -->
        <div class="card mb-3">
            <div class="card-header">
                <button @click="Close()" type="button" class="btn btn-primary"><i class="bi bi-arrow-left"></i></button>
                <template v-if="urlPageDownload">
                    <template v-if="loading">
                        <button @click="Download()" type="button" class="btn btn-danger">
                            <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                        </button>
                    </template>
                    <template v-else>
                        <button @click="Download()" type="button" class="btn btn-danger"><i class="bi bi-cloud-download"></i></button>
                    </template>
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
                    <template v-else-if="episode.stateDownload == null">
                        <div class="progress">
                            <div class="progress-bar bg-info" role="progressbar" :style="'width: 100%'" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100">not yet processed</div>
                        </div>
                    </template>
                </div>
            </div>
        <template v-if="urlExternal == false || urlExternal == null">
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
            episodes:[],
            loading: false,
            conflict: false,
            success:false
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
            $nuxt.$emit('close', this.urlExternal);
        },
        Download(){
            //get api external
            this.loading = true;
            this.$axios.post("https://localhost:44300/animesaturn/download", JSON.stringify({Url: this.urlPageDownload}),
            {
                headers: {
                    'Content-Type': 'application/json'
                }
            })
            .then(rs => {
                if(rs.status == 201){
                    this.success = true;
                    this.Close()
                    $nuxt.$emit("reloadViewDetails", rs.data);
                }else{
                    console.log('error download');
                }
            })
            .catch(error => {
                if(error.message.includes('409')){
                    this.conflict = true;
                }
            })
            .then(() => {
                this.loading = false;
            })
        },
        closeAlert(){
            this.conflict = false;
            this.success = false;
        }
    },

    watch:{
        hide:{
            handler(){
                if(this.urlPage != null)
                {
                    //get api internal
                    this.$axios.get("https://localhost:44300/episode/name/" + this.name)
                        .then(rs => {
                        this.episodes = rs.data
                    });
                }
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