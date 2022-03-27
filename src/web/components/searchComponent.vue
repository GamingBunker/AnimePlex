<template>
    <div class="row center">
        <div class="input-group" style="margin: 5px 0px;">
            <input v-model="value" type="text" class="form-control" placeholder="name...">
            <button @click="submit()" class="btn btn-outline-light" type="button" id="button-addon2">Search my database</button>
            <template v-if="help">
                <button @click="submitExternal()" class="btn btn-danger" type="button" id="button-addon2">Search on AnimeSaturn</button>
            </template>
        </div>
    </div>
</template>

<script>
    export default {
        data(){
            return{
                value:"",
                help:false
            }
        },
        methods:{
            submit(){
                $nuxt.$emit('search', this.value)
                this.help = true
            },
            submitExternal(){
                $nuxt.$emit('searchExternal', this.value)
            }
        }
    }
</script>