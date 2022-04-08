<template>
    <div>
        <span hidden>{{hide}}</span>
        <template v-if="disk">
            <div class="card text-dark bg-light mb-3" style="max-width: 18rem;">
                <div class="card-body" style="padding: 5px;">
                    <div class="d-flex flex-row align-items-center">
                        <div class="">
                            <h3 style="margin: 0px;"><i class="bi bi-device-hdd"></i></h3>
                        </div>
                        <div class="flex-column" style="width: 100%; margin-top: 5px;">
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
                        </div>
                    </div>
                </div>
            </div>
        </template>
    </div>
</template>

<script>
    export default {
        data(){
            return{
                hide:"",
                disk:null,
                spaceFreePercentual:null,

                host:this.$config.ipAPI,
                port:this.$config.portAPI,
                protocol:this.$config.protocolAPI,
            }
        },
        watch:{
            hide:{
                handler(){
                    //get api internal
                    this.$axios.get(`${this.protocol}://${this.host}:${this.port}/disk`)
                        .then(rs => {
                        this.disk = rs.data
                        this.spaceFreePercentual = ((this.disk.diskSizeTotal - this.disk.diskSizeFree)*100)/this.disk.diskSizeTotal;
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