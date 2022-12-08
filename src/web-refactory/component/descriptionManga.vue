<template>
  <template v-if="!checkNull(item)">
    <template v-if="!checkNull(item.name)">
      <div>
        <h6>Name:</h6>
        <p class="card-text">{{ item.name }}</p>
      </div>
      <hr>
    </template>
    <template v-if="!checkNull(item.dateRelease)">
      <div>
        <h6>Date release:</h6>
        <p class="card-text">{{item.dateRelease}}</p>
      </div>
      <hr>
    </template>
    <template v-if="!checkNull(item.artist)">
      <div>
        <h6>Artis:</h6>
        <p class="card-text">{{ item.artist }}</p>
      </div>
      <hr>
    </template>
    <template v-if="!checkNull(item.author)">
      <div>
        <h6>Author:</h6>
        <p class="card-text">{{ item.author }}</p>
      </div>
      <hr>
    </template>
    <template v-if="!checkNull(item.totalVolumes)">
      <div>
        <h6>Total number volumes:</h6>
        <p class="card-text">{{ item.totalVolumes }}</p>
      </div>
      <hr>
    </template>
    <template v-if="!checkNull(item.totalChapters)">
      <div>
        <h6>Total number chapters:</h6>
        <p class="card-text">{{ item.totalChapters }}</p>
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
    <template v-if="!checkNull(item.type)">
      <div>
        <h6>Type:</h6>
        <p class="card-text">{{ item.type }}</p>
      </div>
      <hr>
    </template>
    <template v-if="!checkNull(item.status)">
      <div>
        <h6>Status:</h6>
        <p class="card-text">{{ item.status? 'Terminate' : 'In progress' }}</p>
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
  name: "descriptionManga",
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