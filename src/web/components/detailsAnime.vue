<template>
    <div style="width: 500px; margin: 20px;">
        <!-- auto refresh api  -->
        <input type="hidden" hidden :value="hide">
        
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
                <template v-if="urlPageDownload && exists == false">
                    <template v-if="loading">
                        <button @click="Download()" type="button" class="btn btn-danger">
                            <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                        </button>
                    </template>
                    <template v-else>
                        <button @click="Download()" type="button" class="btn btn-danger"><i class="bi bi-cloud-download"></i></button>
                    </template>
                </template>
                <template v-else-if="exists == null">
                    <template v-if="loading">
                        <button @click="ReDownload()" type="button" class="btn btn-warning">
                            <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                        </button>
                    </template>
                    <template v-else>
                        <template v-if="sure == false">
                            <button @click="ReDownload()" type="button" class="btn btn-warning">Re-Download</button>
                        </template>
                        <template v-else>
                            <button @click="ReDownload()" type="button" class="btn btn-warning"><i class="bi bi-wrench-adjustable-circle"></i> Are you sure?</button>
                        </template>
                    </template>
                </template>
                <template v-else>
                    <button type="button" class="btn btn-success"><i class="bi bi-bookmark-check"></i></button>
                </template>
            </div>
            <div class="card-body">
                <h3 class="card-title">{{name}}</h3>

                <template v-if="urlExternal">
                    <div class="d-grid gap-1">
                        <a class="btn btn-secondary" :href="urlPageDownload" target="_blank" role="button"><i class="bi bi-link"></i></a>
                    </div>
                </template>

                <template v-if="date != 'NaN-NaN-NaN'">
                    <div>
                        <h6>Date release:</h6>
                        <p class="card-text">{{date}}</p>
                    </div>
                <hr>
                </template>
                <template v-if="vote">
                    <div>
                        <h6>Vote:</h6>
                        <p class="card-text">{{vote}}</p>
                    </div>
                <hr>
                </template>
                <template v-if="studio">
                    <div>
                        <h6>Studio:</h6>
                        <p class="card-text">{{studio}}</p>
                    </div>
                <hr>
                </template>
                <template v-if="duration">
                    <div>
                        <h6>Duration:</h6>
                        <p class="card-text">{{duration}}</p>
                    </div>
                <hr>
                </template>
                <template v-if="totalEpisode">
                    <div>
                        <h6>Total number episodes:</h6>
                        <p class="card-text">{{totalEpisode}}</p>
                    </div>
                <hr>
                </template>
                <template v-if="description">
                    <div>
                        <h6>Description:</h6>
                        <p class="card-text">{{description}}</p>
                    </div>
                <hr>
                </template>
                <div v-for="episode in episodes" :key="episode.id">
                    {{episode.idAnime}} episode: {{episode.numberEpisodeCurrent}}
                    <template v-if="episode.stateDownload == 'downloading'">
                        <div class="progress">
                            <div class="progress-bar progress-bar-striped progress-bar-animated" role="progressbar" :style="'width: '+episode.percentualDownload+'%'" :aria-valuenow="episode.percentualDownload" aria-valuemin="0" aria-valuemax="100">{{episode.percentualDownload}}%</div>
                        </div>
                    </template>
                    <template v-else-if="episode.stateDownload == 'completed'">
                        <div class="progress">
                            <div class="progress-bar bg-success" role="progressbar" :style="'width: '+episode.percentualDownload+'%'" :aria-valuenow="episode.percentualDownload" aria-valuemin="0" aria-valuemax="100">completed</div>
                        </div>
                    </template>
                    <template v-else-if="episode.stateDownload == 'failed'">
                        <div class="progress">
                            <div class="progress-bar bg-danger" role="progressbar" :style="'width: 100%'" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100">failed</div>
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
            success:false,
            
            host:this.$config.ipAPI,
            port:this.$config.portAPI,
            protocol:this.$config.protocolAPI,

            sure:false
        }
    },
    props:{
        name:String,
        description:String,
        date:String,
        vote:String,
        studio:String,
        image:String,
        finish:Boolean,
        urlPage:String,
        duration:String,
        totalEpisode:Number,
        urlPageDownload:String,
        urlExternal:Boolean,
        exists:Boolean
    },

    methods: {
        ConvertBase64(imgBase64){
            var buff = new Buffer(imgBase64);
            return  buff.toString();
        },
        Close(){
            $nuxt.$emit('close', this.urlExternal);
        },
        ReDownload(){
            if(this.sure == false)
            {
                this.sure = true;
                return
            }
            this.loading = true;
            this.$axios.put(`${this.protocol}://${this.host}:${this.port}/animesaturn/redownload`, JSON.stringify(this.episodes),
                {
                    headers: {
                        'Content-Type': 'application/json'
                    }
                })
                .then(rs => {
                    if(rs.status != 200){
                        console.log('error reset');
                    }
                })
                .catch(error => {
                    if(error.message.includes('500')){
                        console.log('error internal server api');
                    }
                })
                .then(() => {
                    this.loading = false;
                    this.sure = false;
                })
        },
        Download(){
            //get api external
            this.loading = true;
            this.$axios.post(`${this.protocol}://${this.host}:${this.port}/animesaturn/download`, JSON.stringify({Url: this.urlPageDownload}),
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
                    this.$axios.get(`${this.protocol}://${this.host}:${this.port}/episode/name/${this.name}`)
                        .then(rs => {
                        this.episodes = rs.data
                    });
                }
                setTimeout(() => {
                    this.hide = new Date();
                }, 3000);
            },
            immediate: true
        },
        sure:{
            handler(){
                setTimeout(() => {
                    if(this.sure == true)
                        this.sure = false
                }, 1000);
            }
        }
    }
}
</script>