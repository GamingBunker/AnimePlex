<template>
    <div class="container d-flex justify-content-center" style="margin-top: 100px;">
        {{load()}}
        <template v-if="this.$route.query.type === 'anime' && data != null">
            <video id="my-video" class="video-js" width="100%" controls>
                <source :src="getUrl(data.episodePath)" type="video/mp4">
            </video>
        </template>
    </div>
</template>

<script>
    export default {
        data(){
            return{
                data:null,
                hide:"",
                
                host: this.$config.ipAPI,
                port: this.$config.portAPI,
                protocol: this.$config.protocolAPI,
            }
        },
        methods:{
            load(){
                if(this.$route.query.type === "anime" && this.data == null)
                {
                    //get api internal
                    this.$axios.get(`${this.protocol}://${this.host}:${this.port}/episode/register/episodeid/${this.$route.params.room}`)
                        .then(rs => {
                        this.data = rs.data
                    });
                    
                }else if(this.$route.query.type === "manga" && this.data == null){
                    //get api internal
                    this.$axios.get(`${this.protocol}://${this.host}:${this.port}/chapter/register/chapterid/${this.$route.params.room}`)
                        .then(rs => {
                        this.data = rs.data
                    });
                }
            },
            getUrl(url){
                //try windows
                /*if(url[1] == ':'){
                    url = url.replace(/\\/g, '\\\\');
                    url = url.replace(/\//g, '\\\\');
                }*/

                var src = url.split('video');
                
                return require('../../video'+src[1])
            }
        }
    }
</script>