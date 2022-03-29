<template>
    <div class="row center">
        <div class="input-group" style="margin: 5px 0px;">
            <input v-model="value" type="text" class="form-control" placeholder="name...">
            <button @click="searchInternalAll()" class="btn btn-warning" type="button" id="button-addon2">Search all my database</button>
            <button @click="searchInternal()" class="btn btn-light" type="button" id="button-addon2">Search my database</button>
            <button @click="submitExternal()" class="btn btn-danger" type="button" id="button-addon2">Search on AnimeSaturn</button>
        </div>
    </div>
</template>

<script>
    export default {
        data(){
            return{
                value:""
            }
        },
        methods:{
            searchInternal(){
                $nuxt.$emit('search', this.value)
            },
            submitExternal(){
                $nuxt.$emit('searchExternal', this.value)
            },
            searchInternalAll(){
                $nuxt.$emit('searchall', this.value)
            }
        }
    }
</script>