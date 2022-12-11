<template>
  <template v-if="!checkNull(item)">
    <template v-if="!checkNull(item.name)">
      <div>
        <h6>Name:</h6>
        <p class="card-text">{{ item.name }}</p>
      </div>
      <hr>
    </template>
    <template v-if="!checkNull(item.dateRelease) && item.dateRelease !== 'NaN-NaN-NaN'">
      <div>
        <h6>Date release:</h6>
        <p class="card-text">{{new Date(item.dateRelease).toLocaleDateString()}}</p>
      </div>
      <hr>
    </template>
    <template v-if="!checkNull(item.vote)">
      <div>
        <h6>Vote:</h6>
        <p class="card-text">{{ item.vote }}</p>
      </div>
      <hr>
    </template>
    <template v-if="!checkNull(item.studio)">
      <div>
        <h6>Studio:</h6>
        <p class="card-text">{{ item.studio }}</p>
      </div>
      <hr>
    </template>
    <template v-if="!checkNull(item.durationEpisode)">
      <div>
        <h6>Duration:</h6>
        <p class="card-text">{{ item.durationEpisode }}</p>
      </div>
      <hr>
    </template>
    <template v-if="!checkNull(item.episodeTotal)">
      <div>
        <h6>Total number episodes:</h6>
        <p class="card-text">{{ item.episodeTotal }}</p>
      </div>
      <hr>
    </template>
    <template v-if="!checkNull(item.description)">
      <div>
        <h6>Description:</h6>
        <p class="card-text">{{ item.description }}</p>
      </div>
      <hr>
    </template>
    <template v-if="!checkNull(item.finish)">
      <div>
        <h6>Status:</h6>
        <p class="card-text">{{ item.finish? 'Terminate' : 'In progress' }}</p>
      </div>
      <hr>
    </template>
    <template v-if="!checkNull(item.urlPageDownload)">
        <a :href="item.urlPageDownload" target="_blank">
          <v-btn
              class="btn-link"
          >
            <v-icon
                size="15"
                color="white"
            >
              $link
            </v-icon>
          </v-btn>
        </a>
    </template>
  </template>
</template>

<script>
import lodash from "../mixins/lodash";
export default {
  name: "descriptionAnime",
  props: [
    'item'
  ],
  mixins:[
    lodash
  ]
}
</script>
<style lang="scss" scoped>
.btn-link{
  width: 100%;
  background-color: #6c757d;
  text-align: center;
  padding: 5px;
  border-radius: 5px;
}
</style>