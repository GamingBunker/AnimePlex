<template>
    <div class="card" style="width: 18rem;">
    <img :src="'data:image/jpg;base64,'+ConvertBase64(image)" class="card-img-top">
        <div class="card-body">
            <h5 class="card-title">{{name}}</h5>
            <a @click="viewDetails()" class="btn btn-primary">Details</a>
        </div>
    </div>
</template>

<script>
    export default {
        props:{
            name:String,
            image:String
        },
        methods: {
            ConvertBase64(imgBase64){
                var buff = new Buffer(imgBase64);
                return  buff.toString()
            },
            viewDetails(){
                $nuxt.$emit('viewDetails', this.name)
            }
        }
    }
</script>

<style lang="scss" scoped>

</style>