<template>
    <div>
        <span hidden>{{time}}</span>
            <div class="card text-dark bg-light mb-3" style="max-width: 18rem;">
                <div class="card-header">
                    Space Disk
                </div>
                <div class="card-body" style="padding: 5px;">
                    <div class="d-flex flex-row align-items-center">
                        <div class="">
                            <h3 style="margin: 0px;"><i class="bi bi-device-hdd"></i></h3>
                        </div>
                        <div class="flex-column" style="width: 100%; margin-top: 5px;">
                            <template v-if="disk != null && time - disk.lastCheck <= 30000">
                                <div class="progress" style="padding: 0px;">
                                    <template v-if="spaceFreePercentual < 75">
                                        <div class="progress-bar bg-success" role="progressbar" :style="'width: '+spaceFreePercentual+'%;'" :aria-valuenow="spaceFreePercentual" aria-valuemin="0" aria-valuemax="100">{{spaceFreePercentual}}%</div>
                                    </template>
                                    <template v-else-if="spaceFreePercentual >= 75 && spaceFreePercentual <90">
                                        <div class="progress-bar bg-warning" role="progressbar" :style="'width: '+spaceFreePercentual+'%;'" :aria-valuenow="spaceFreePercentual" aria-valuemin="0" aria-valuemax="100">{{spaceFreePercentual}}%</div>
                                    </template>
                                    <template v-else>
                                        <div class="progress-bar bg-danger" role="progressbar" :style="'width: '+spaceFreePercentual+'%;'" :aria-valuenow="spaceFreePercentual" aria-valuemin="0" aria-valuemax="100">{{spaceFreePercentual}}%</div>
                                    </template>
                                </div>
                                <div class="d-flex flex-row justify-content-between">
                                    <div>{{disk.diskSizeTotal - disk.diskSizeFree}} GB</div>
                                    <div>{{disk.diskSizeTotal}} GB</div>
                                </div>
                            </template>
                            <template v-else>
                                <div class="progress">
                                    <div class="progress-bar bg-secondary" role="progressbar" style="width: 100%" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100"><b>UNKNOW</b></div>
                                </div>
                            </template>
                        </div>
                    </div>
                </div>
            </div>
    </div>
</template>

<script>
    export default {
        data(){
            return{
                time:"",
                disk:null,
                spaceFreePercentual:null,

                host:this.$config.ipAPI,
                port:this.$config.portAPI,
                protocol:this.$config.protocolAPI,
            }
        },
        watch:{
            time:{
                handler(){
                    //get api internal
                    this.$axios.get(`${this.protocol}://${this.host}:${this.port}/disk`)
                    .then(rs => {
                        this.disk = rs.data
                        this.spaceFreePercentual = ((this.disk.diskSizeTotal - this.disk.diskSizeFree)*100)/this.disk.diskSizeTotal;
                    })
                    .catch(error => {
                        this.disk = null;
                    })

                    setTimeout(() => {
                        this.time = new Date().getTime();
                    }, 3000);
                },
                immediate: true
            }
        }
    }
</script>