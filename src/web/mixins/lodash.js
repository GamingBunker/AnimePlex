import _ from 'lodash';

export default {
  methods:{
      checkNull(value){
          return _.isNil(value)
      }
  }
}