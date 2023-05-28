<template>
    <el-progress style="width:300px" :percentage="percentage" />
</template>
<script>
import Fakeprogress from 'fake-progress'
export default {
    name: 'LoadingV',
    data() {
        return {
            timer: {},
            percentage: 0,
            fake: new Fakeprogress({
                timeConstant: 2000
            })
        }
    },
    methods: {
        end() {
            // this.percentage = 100
            this.fake.end()
            clearInterval(this.timer)
        }
    },
    mounted() {

        this.fake.progress = 0
        this.fake.start()
        this.timer = setInterval(() => {
            this.percentage = parseInt(this.fake.progress * 100)
            // console.log(this.fake.progress)
        }, 50);
        this.$mybus.on("loading", this.end())
    },
    beforeUnmount() {
        this.$mybus.off("loading")
        this.end()
    }
}
</script>
 <!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
h3 {
    margin: 40px 0 0;
}
ul {
    list-style-type: none;
    padding: 0;
}
li {
    display: inline-block;
    margin: 0 10px;
}
a {
    color: #42b983;
}
</style>
 