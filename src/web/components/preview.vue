<template>
    <div class="card" style="width: 18rem; padding: 0px; margin-right: 7px; margin-left: 3px; margin-bottom:10px;">
        <div class="row">
            <div class="col">
                <template v-if="urlExternal == false">
                    <img :src="'data:image/jpg;base64,'+ConvertBase64(image)" class="card-img-top rounded-top" width="400" height="450" style="object-fit: cover;">
                </template>
                <template v-else>
                    <img :src="image" class="card-img-top rounded-top" width="400" height="450" style="object-fit: cover;">
                </template>
            </div>
        </div>
        <div class="card-body">
            <div class="row">
                <div class="col">
                    <h5 class="card-title">{{name}}</h5>
                </div>
            </div>
            <div class="row">
                <div class="col">
                    <a @click="viewDetails()" class="btn btn-primary">Details</a>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    export default {
        props:{
            name:String,
            image:String,
            urlExternal:Boolean,
            positionCurrent:Number,
            typeView:String
        },
        methods: {
            ConvertBase64(imgBase64){
                var buff = new Buffer(imgBase64);
                return  buff.toString()
            },
            viewDetails(){
                $nuxt.$emit('viewDetails', this.positionCurrent, this.typeView)
            }
        }
    }
</script>