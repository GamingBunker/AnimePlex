<template>
    <div class="card" :style="`width: 110px; background-color:${background};`">
        <div style="margin: auto; padding: 5px;">
            <img src="../assets/img/profile.png" width="50"/>
        </div>
        <div class="card-body" style="padding: 1px 5px; margin: auto;">
            {{nickname}}
        </div>
    </div>
</template>

<script>
    export default {
        props:{
            nickname:String,
            current:Boolean
        },
        data(){
            return{
                background:'white'
            }
        },
        mounted(){
            if(this.current == true)
                this.background = 'greenyellow'
        }
    }
</script>

<style>
.card-body{
    font-size: x-small;
}
</style>