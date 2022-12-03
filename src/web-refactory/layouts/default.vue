<template>
  <v-app>
      <v-navigation-drawer
        expand-on-hover
        color="#363636"
        rail
        permanent
      >
        <v-list
          density="compact"
          class="list-items"
          :selected="select"
          nav
        >
          <v-list-item
            v-for="item in items"
            :key="item.id"
            :value="item"
            @click="setTypeComponent(item.id)"
          >
            <template
                v-slot:prepend
            >
              <v-icon>{{item.icon}}</v-icon>
            </template>

            <v-list-item-title v-text="item.text" />
            <v-list-item-action

            />
          </v-list-item>
        </v-list>
      </v-navigation-drawer>
    <v-main>
      <NuxtPage />
    </v-main>
  </v-app>
</template>
<script>
import lodash from "../mixins/lodash";
import {useStore} from "../store";
export default {
  name: "index",
  mixins:[
    lodash
  ],
  setup(){
    const store = useStore();

    return {store}
  },
  data(){
    return{
      items:[
        {
          id:'all',
          text:'All local DB',
          icon:'$database'
        },
        {
          id:'search-local',
          text:'Search local DB',
          icon:'$search'
        },
        {
          id:'search-animesaturn',
          text:'Search on AnimeSaturn',
          icon:'$planet'
        },
        {
          id:'search-mangaworld',
          text:'Search on MangaWorld',
          icon:'$book'
        },
      ],
      select:[]
    }
  },
  methods:{
    setTypeComponent(id){
      this.store.setCurrentSelectSearch(id);
    }
  }
}
</script>

<style lang="scss" scoped>
.list-items{
  color: white !important;
}
</style>