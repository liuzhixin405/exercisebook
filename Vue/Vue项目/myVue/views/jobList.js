jobList={
    template:`
    <div class="job-list">
    <table>
        <tr v-for="job in jobs" :key="job.id">
            <td classs="job-info">
                <p class="j-name">{{job.jobName}}</p>
                <p class="j-pay">{{job.jobPay}}}}</p>
                <p class="j-welfare">
                    <span v-for="welfare in getWelfare(job.welfare)">{{welfare}}}</span>
                </p>
            </td>
            <td class="job-position">
                <p>{{job.companyAddress}}}</p>
                <p>{{job.city.city}}|{{job.education}}|{{job.workExperience}}</p>
                <p>{{job.publishTime}}}</p>
            </td>
            <td class="job-request">
                <button>
                    申请
                </button>
            </td>
        </tr>
    </table>
    </div>
    `,
    data(){
        return{
            jobs:[],
        }
    },
    mounted(){
        this.getJobs();
    },
    methods:{
        getJobs(){
            axios.get("http://localhost:5009/job").then(res=>{
                console.log(res);
                this.jobs=res.data;
            });
        },
        getWelfare(welfare){
            return welfare.split(",");
        }
    }
}