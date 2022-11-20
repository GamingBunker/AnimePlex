<template>
  <v-app>
      <v-navigation-drawer
        expand-on-hover
        color="#363636"
        rail
      >
        <v-list
          density="compact"
          class="list-items"
          nav
        >
          <v-list-item
            title="All local DB"
            prepend-icon="$database"
            value="test"
          />
          <v-list-item
              title="Search local DB"
              prepend-icon="$search"
              value="test1"
          />
          <v-list-item
              title="Search on AnimeSaturn"
              prepend-icon="$planet"
              value="test2"
          />
          <v-list-item
              title="Search on MangaWorld"
              prepend-icon="$book"
              value="test3"
          />
        </v-list>
      </v-navigation-drawer>
    <v-main
    >
      <NuxtPage />
    </v-main>
  </v-app>
</template>

<script>
export default {
  name: "index"
}
</script>

<style lang="scss" scoped>
.list-items{
  color: white !important;
}
</style>