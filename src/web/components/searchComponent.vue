<template>
    <div class="row center">
        <div class="input-group mb-3">
            <input v-model="value" type="text" class="form-control" placeholder="search...">
            <button @click="submit()" class="btn btn-outline-primary" type="button" id="button-addon2">Search</button>
            <template v-if="help">
                <button @click="submitExternal()" class="btn btn-danger" type="button" id="button-addon2">Not found?</button>
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

<style lang="scss" scoped>

</style>