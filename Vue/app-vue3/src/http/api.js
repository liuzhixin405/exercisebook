import $http from './index.js'
export const getCourseList =(data)=>{
    return $http.get('http://jsonplaceholder.typicode.com/posts',data)
} 
export const login = (data)=>{
    return $http.get('http://jsonplaceholder.typicode.com/posts',data)
}