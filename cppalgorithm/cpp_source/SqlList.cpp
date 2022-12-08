#define ListSize 100
typedef struct SqlList
{
    int List[ListSize];
    int length;
}SeqList;

void InitList(SeqList *L){
    L->length=0;
}