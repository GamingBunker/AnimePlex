import {defineEventHandler} from "h3";
import axios from "axios";

export default defineEventHandler(async (event) => {
    const {data} = await axios.get('http://145.110.1.253:8090/check');
    return data;
})