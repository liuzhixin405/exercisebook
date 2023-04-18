package main

import (
	"fmt"
	"log"
	"net/http"
	"time"

	"github.com/chatgp/gpt3"
)

type ChatHandler struct{}

func (h *ChatHandler) ServeHTTP(w http.ResponseWriter, r *http.Request) {
	queryParameters := r.URL.Query()
	problem := queryParameters.Get("q")
	if len(problem) == 0 {
		fmt.Fprintf(w, "你还没问问题呢。比如这样问:http://www.eiza.net/chat?q=今天天气如何?")
		return
	}

	cli := getClient()

	uri := "/v1/chat/completions"
	params := map[string]interface{}{
		"model": "gpt-3.5-turbo",
		"messages": []map[string]interface{}{
			{"role": "user", "content": problem},
		},
	}

	res, err := cli.Post(uri, params)
	if err != nil {
		log.Fatalf("request api failed: %v", err)
	}

	message := res.Get("choices.0.message.content").String()

	fmt.Fprintf(w, message)
}

func main() {
	handler := ChatHandler{}
	server := http.Server{
		Addr: "127.0.0.1:80",
	}
	http.Handle("/chat", &handler)
	http.Handle("/", &handler)
	server.ListenAndServe()

}

func getClient() *gpt3.Client {
	apiKey := "key"
	cli, _ := gpt3.NewClient(&gpt3.Options{
		ApiKey:  apiKey,
		Timeout: 30 * time.Second,
		Debug:   true,
	})
	return cli
}
