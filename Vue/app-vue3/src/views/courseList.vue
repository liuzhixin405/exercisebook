<template>
  <div class="select-box">
    <div>
      <span>id:</span>
      <el-select v-model="option.id" placeholder="请选择id">
        <el-option
          v-for="item in [1,2,3,4,5,6,7]"
          :key="item"
          :label="item"
          :value="item"
        >
        </el-option>
      </el-select>
    </div>
    <div>
      <span>标题:</span>
     <el-input v-model="option.title" placeholder="请输入标题"></el-input>
    </div>
     <div>
    <el-button @click="getTabList" type="primary">查询</el-button>
  </div>
  </div>
 
  <el-table border :data="data.table[option.page]" style="width: 100%">
    <el-table-column prop="title" label="标题"> </el-table-column>
    <el-table-column prop="body" label="内容"> </el-table-column>
    <el-table-column prop="id" label="id"> </el-table-column>
  </el-table>
  <el-pagination
    @current-change="handleCurrentChange"
    :current-page="data.page"
    layout="total, prev, pager, next, jumper"
    :total="data.total"
  >
  </el-pagination>
</template>

<script>
import { reactive } from "vue";
import { getCourseList } from "../http/api";
export default {
  name: "CourseList",
  setup() {
    let data = reactive({
      table: [],
      psages: 0,
      total: 0,
    });

  let option=reactive({
    id:'',
    title:'',
    page: 0,

  })

    let getCourseListFun = async (obj) => {
      let arr = await getCourseList(obj),
        newArr = [];
      data.total = arr.length;
      //   [1,2,3,4,5,6,7]   [10,11,12----100]
      //   [[1,2],[3,4],[5,6],[7]]
      //split(index,10)==[0-10]
      for (let index = 0; index < arr.length; index++) {
        let obj = arr.splice(index, 10);
        newArr.push(obj);
        index += 10;
      }
      data.table = newArr;
    };
    getCourseListFun();

    let handleCurrentChange = (index) => {
      option.page = index;
      // getCourseListFun({page:data.page})
    };

    let getTabList=()=>{
         console.log(option) 
    }

    return {
      data,
      handleCurrentChange,
      getTabList,
      option
    };
  },
};
</script>
<style lang="scss" scoped>
  .select-box{
       display: flex;
      align-items: center;
      width: 100%;
      margin-bottom: 10px;
    >div{
      margin-right: 10px;
      width: 30%;
      display: flex;
      align-items: center;
      span{
        width: 50px;
      }

    }
  }
</style>
