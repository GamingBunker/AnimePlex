<template>
    <div>
        <span hidden>{{time}}</span>
        <div class="card text-dark bg-light mb-3 d-flex flex-column" style="max-width: 18rem;">
            <div class="card-header">
                Health Services
            </div>
            <template v-if="healthServices">
                <div v-for="service in healthServices" :key="service.nameService">
                    <div class="card-body" style="padding: 5px;">
                        <div class="d-flex flex-row align-items-center text-center">
                            <div class="col">
                                <label>{{service.nameService}}</label>
                            </div>
                            <div class="col" style="padding: 5px;">
                                <template v-if="(time - service.lastCheck) <= (service.interval + 5000)">
                                    <div class="alert alert-success" role="alert"  style="width: 100px; margin: 0px;">
                                        <i class="bi bi-emoji-smile"></i>
                                        <span>Ok!</span>
                                    </div>
                                </template>
                                <template v-else-if="(time - service.lastCheck) > (service.interval + 5000) && (time - service.lastCheck) <= (service.interval + 30000)">
                                    <div class="alert alert-warning" role="alert" style="width: 110px; margin: 0px;">
                                        <i class="bi bi-emoji-neutral"></i>
                                        <span>Alive?</span>
                                    </div>
                                </template>
                                <template v-else>
                                    <div class="alert alert-danger" role="alert" style="width: 110px; margin: 0px;">
                                        <i class="bi bi-emoji-dizzy"></i>
                                        <span>Death</span>
                                    </div>
                                </template>
                            </div>
                        </div>
                    </div>
                </div>
            </template>
            <template v-else>
                <div class="card-body" style="padding: 5px;">
                    <div class="d-flex flex-row align-items-center text-center">
                        <div class="col">
                            <label>API</label>
                        </div>
                        <div class="col" style="padding: 5px;">                            
                            <div class="alert alert-danger" role="alert" style="width: 100px; margin: 0px;">
                                <i class="bi bi-emoji-dizzy"></i>
                                <span>Death</span>
                            </div>
                        </div>
                    </div>
                </div>
            </template>
        </div>
    </div>
</template>

<script>
    export default {
        data(){
            return{
                time:"",
                healthServices:"",

                host:this.$config.ipAPI,
                port:this.$config.portAPI,
                protocol:this.$config.protocolAPI,
            }
        },
        methods:{
            getApi()
            {
                //get api internal
                this.$axios
                .get(`${this.protocol}://${this.host}:${this.port}/health`)
                .then(rs => {
                    this.healthServices = rs.data
                })
                .catch(error => {
                    this.healthServices = null;
                })
            }
        },
        watch:{
            time:{
                handler(){
                    this.getApi()

                    setTimeout(() => {
                        this.time = new Date().getTime();
                    }, 1000);
                },
                immediate: true
            }
        }
    }
</script>

<style scoped>

</style>