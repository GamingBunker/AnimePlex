<template>
  <v-progress-linear
      v-model="item.percentualDownload"
      :striped="item.percentualDownload < 100"
      :color="getStatus"
      height="25"
      striped
  >
    <template v-slot:default="{ value }">
      <template v-if="checkNull(item.stateDownload)">
        <strong>NOT YET PROCESSED</strong>
      </template>
      <template v-else-if="item.stateDownload.toUpperCase() === 'DOWNLOADING'">
        <strong>{{ this.item.percentualDownload }} %</strong>
      </template>
      <template v-else>
        <strong style="color: white">{{ this.item.stateDownload.toUpperCase() }}</strong>
      </template>
    </template>
  </v-progress-linear>
</template>

<script>
import lodash from '/mixins/lodash'

export default {
  name: "progressBar",
  props: [
    'item',
    'type'
  ],
  mixins: [
    lodash
  ],
  computed: {
    getStatus() {
      switch (this.item.stateDownload) {
        case 'downloading':
          return 'primary';
        case 'conversioning':
          return 'info';
        case 'completed':
          return 'success';
        case 'failed':
          return 'error';
        case 'wait conversion':
          return 'grey';
        case 'pending':
          return 'warning';
        default:
          return 'grey';
      }
    }
  }
}
</script>